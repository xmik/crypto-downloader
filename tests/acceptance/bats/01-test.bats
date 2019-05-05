load '/opt/bats-support/load.bash'
load '/opt/bats-assert/load.bash'

executable="./src/CryptoDownloaderConsole/bin/Release/netcoreapp2.1/CryptoDownloaderConsole.dll"

@test "--help" {
  run dotnet "${executable}" --help
  assert_line --partial "get-batch-number"
  assert_line --partial "get-old-batch-number"
  assert_line --partial "list"
  assert_line --partial "get"
  assert_line --partial "Download historical prices"
  assert_equal "$status" 0
}
@test "--invalidoption" {
  run dotnet "${executable}" --invalidoption
  assert_line --partial "Verb '--invalidoption' is not recognized"
  assert_equal "$status" 1
}
@test "batch-to-date-range 0" {
  run dotnet "${executable}" batch-to-date-range 0
  assert_line --partial "Expected batch number >0, but was: 0"
  assert_equal "$status" 5
}
@test "batch-to-date-range 1" {
  run dotnet "${executable}" batch-to-date-range 1
  assert_line "Start: 2019-02-10 00:00:00 End: 2019-02-16 23:59:59"
  assert_equal "$status" 0
}
@test "batch-to-date-range 12" {
  run dotnet "${executable}" batch-to-date-range 12
  assert_line "Start: 2019-04-28 00:00:00 End: 2019-05-04 23:59:59"
  assert_equal "$status" 0
}
@test "get-batch-number" {
  run dotnet "${executable}" get-batch-number
  refute_line --partial "Exception"
  assert_equal "$status" 0
}
@test "get-old-batch-number" {
  run dotnet "${executable}" get-old-batch-number
  refute_line --partial "Exception"
  assert_equal "$status" 0
}
@test "list" {
  run dotnet "${executable}" list
  assert_line --partial "BTC_ARDR"
  assert_line --partial "BTC_DOGE"
  assert_line --partial "USDT_ETC"
  assert_line --partial "USDC_BTC"
  # we don't want the trailing coma
  refute_output --partial "USDC_BTC,"
  assert_line --partial "ETH_LSK"
  assert_line --partial "XMR_BCN"
  assert_equal "$status" 0
}
@test "list --invalidoption" {
  run dotnet "${executable}" list --invalidoption
  assert_line --partial "Option 'invalidoption' is unknown"
  assert_equal "$status" 1
}
