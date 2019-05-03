load '/opt/bats-support/load.bash'
load '/opt/bats-assert/load.bash'

executable="./src/CryptoDownloaderConsole/bin/Release/netcoreapp2.1/CryptoDownloaderConsole.dll"

@test "get --help" {
  run dotnet "${executable}" get --help
  assert_line --partial "--instruments"
  assert_line --partial "--batch"
  assert_line --partial "--directory"
  assert_equal "$status" 0
}
@test "get --invalidoption" {
  run dotnet "${executable}" get --invalidoption
  assert_line --partial "Option 'invalidoption' is unknown"
  assert_equal "$status" 1
}
@test "get with invalid batch number -2" {
  rm -rf ./tests/integration/testdata
  run dotnet "${executable}" get --instruments="USDC_BTC" --batch=-2 --directory=./tests/integration/testdata
  assert_line --partial "Expected batch number >0, but was: -2"
  assert_equal "$status" 5
  rm -rf ./tests/integration/testdata
}
@test "get with not existing instrument" {
  rm -rf ./tests/integration/testdata
  run dotnet "${executable}" get --instruments="idonotexist" --batch=1 --directory=./tests/integration/testdata
  assert_line --partial "Invalid currency pair"
  assert_equal "$status" 5
  rm -rf ./tests/integration/testdata
}
@test "get with valid parameters (1st batch of 1 instrument)" {
  rm -rf ./tests/integration/testdata
  run dotnet "${executable}" get --instruments="USDC_BTC" --batch=1 --directory=./tests/integration/testdata
  assert_output --partial "Downloading instrument 1/1: USDC_BTC"
  assert_output --partial "Finished download with success"
  refute_output --partial "Exception"
  refute_output --partial "Error"
  assert_equal "$status" 0

  run test -f ./tests/integration/testdata/1/USDC_BTC.csv
  assert_equal "$status" 0
  # test that file size is not 0
  run stat -c '%s' ./tests/integration/testdata/1/USDC_BTC.csv
  refute_output "0"
  assert_equal "$status" 0

  rm -rf ./tests/integration/testdata
}
@test "get with valid parameters (1st batch of all instruments with dryrun)" {
  instruments=$(dotnet ${executable} list)
  run dotnet "${executable}" get --instruments=${instruments} --batch=1 --directory=./tests/integration/testdata --dryrun=true
  assert_output --partial "Downloading instrument 1/"
  assert_output --partial "Downloading instrument 10/"
  assert_output --partial "Downloading instrument 125/"
  assert_output --partial "Finished download with success"
  refute_output --partial "Exception"
  refute_output --partial "Error"
  refute_output --partial "ERROR"
  assert_equal "$status" 0
}
