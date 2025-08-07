import argparse
from argparse import Action
from dataclasses import dataclass
import os
import re
from typing import Iterable, List


MINIMUM_PADDING = 3
HIDDEN_FILE_PREFIX = "."


def main():
    """
    This program renames all files in the specified folder to be compatible with the TTS Importer.

    Run it with -h to see the usage instructions.

    It assumes all files in the folder represent double-sided tokens and are sorted in "natural" order. It assumes a
    token's front image comes immediately before it's rear image.

    For example, given a directory with the following files:
    GE PZL B1_ART_V0.2.png
    GE PZL B1_ART_V0.22.png
    GE PZL B1_ART_V0.23.png
    GE PZL B1_ART_V0.24.png
    ...

    They would be renamed to:
    001.A.png
    001.B.png
    002.A.png
    002.B.png
    ...
    """
    # Get program arguments:
    args = ProgramArguments.get_program_arguments()
    print(f"Running script {os.path.basename(__file__)} with arguments:")
    print(args)
    print()

    # Change working directory to the target directory:
    if args.target_directory:
        os.chdir(args.target_directory)

    working_directory = os.getcwd()
    print(f"Starting Directory: {working_directory}")
    print()

    # Recursively walk through all directories.
    # Note: os.walk automatically recurses for us, so we don't need to call it on each subdirectory.
    directories_with_error = []
    for current_directory, subdirectories, files in os.walk(working_directory):
        os.chdir(current_directory)
        if args.dry_run:
            print(f"Validating filenames in {current_directory}")
        else:
            print(f"Renaming filenames in: {current_directory}")

        if not rename_filenames(files, args.output_prefix, args.dry_run):
            directories_with_error.append(current_directory)

        print()
        if not args.recursive:
            break

    print()
    num_directories_with_error = len(directories_with_error)
    if num_directories_with_error == 0:
        if args.dry_run:
            print(f"SUCCESS SUCCESS SUCCESS ===> There were no errors detected in any directory.")
        else:
            print(f"SUCCESS SUCCESS SUCCESS ===> Renamed files in all directories.")
    else:
        action = '' if args.dry_run else 'Renamed files in all other directories.'
        if num_directories_with_error == 1:
            print(f"ERROR ERROR ERROR ===> There was 1 directory with an error. Scroll up to see the issues in that "
                  f"directory. {action}")
        else:
            print(f"ERROR ERROR ERROR ===> There were {num_directories_with_error} directories with an error. Scroll "
                  f"up to see the issues in each directory. {action}")


@dataclass
class ProgramArguments:
    target_directory: str
    output_prefix: str
    recursive: bool
    dry_run: bool

    @staticmethod
    def get_program_arguments() -> 'ProgramArguments':
        """Get a new instance of ProgramArguments by parsing the program's arguments."""
        parser = ProgramArguments.get_argument_parser()
        args = parser.parse_args()
        return ProgramArguments(**vars(args))

    @staticmethod
    def get_argument_parser() -> argparse.ArgumentParser:
        parser = argparse.ArgumentParser(description='See the script for details.')
        parser.add_argument('-t', '--target-directory', default='', type=str,
                            help='Absolute path to the directory to rename the files in. Defaults to the current '
                                 'directory.')
        parser.add_argument('-o', '--output-prefix', default='', type=str,
                            help='Prefix to add to the renamed files. Defaults to no prefix.')
        parser.add_argument('-r', '--recursive', action='store_true',
                            help='If true, executes recursively inside each folder. Defaults to False.')
        parser.add_argument('-d', '--dry-run', action='store_true',
                            help='If true, checks filenames without renaming them. It\'s a good idea to do a dry-run '
                                 'before renaming files in case there are any naming issues. Defaults to False.')
        return parser


def rename_filenames(filenames: List[str], prefix: str, dry_run: bool = False) -> bool:
    # Exclude hidden files:
    files = [
        filename for filename
        in filenames
        if not filename.startswith(HIDDEN_FILE_PREFIX)
    ]

    if is_odd(len(files)):
        if dry_run:
            print(f"ERROR: There are an odd number of files.")
        else:
            print(f"ERROR: There are an odd number of files. Skipping this directory.")
        return False

    if dry_run:
        return True

    files = natural_sort(files)

    # Rename files:
    padding_width = max(MINIMUM_PADDING, num_digits(len(files)))
    token_number = 1
    for side_a_file, side_b_file in group_as_pairs(files):
        final_prefix = f"{prefix}{str(token_number).zfill(padding_width)}"
        side_a_new_name = f"{final_prefix}.A{get_file_extension(side_a_file)}"
        side_b_new_name = f"{final_prefix}.B{get_file_extension(side_b_file)}"

        os.rename(side_a_file, side_a_new_name)
        print(f"Renamed {side_a_file} to {side_a_new_name}")

        os.rename(side_b_file, side_b_new_name)
        print(f"Renamed {side_b_file} to {side_b_new_name}")

        token_number += 1

    print(f"SUCCESS. Renamed all files in this directory.")
    return True


def is_odd(number: int) -> bool:
    return number % 2 != 0


# Source: https://docs.python.org/3/library/itertools.html#recipes
# Source: https://stackoverflow.com/a/11350430/5886427
def group_as_pairs(iterable: Iterable) -> Iterable:
    """Collect data into non-overlapping fixed-length chunks or blocks."""
    # group_as_pairs('ABCDEF') → AB CD EF
    # group_as_pairs('ABCDEFG') → AB CD EF ValueError
    iterators = [iter(iterable)] * 2
    return zip(*iterators, strict=True)


def num_digits(number: int) -> int:
    return len(str(number))


def get_file_extension(filename: str) -> str:
    filename, file_extension = os.path.splitext(filename)
    return file_extension


# Source: https://stackoverflow.com/a/4836734/5886427
def natural_sort(iterable: Iterable) -> list:
    """Return A new list sorted in natural order."""
    convert = lambda text: int(text) if text.isdigit() else text.lower()
    alphanum_key = lambda key: [convert(c) for c in re.split('([0-9]+)', key)]
    return sorted(iterable, key=alphanum_key)


if __name__ == '__main__':
    main()
